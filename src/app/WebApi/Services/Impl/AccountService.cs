using Akka.Actor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Model.Accounts;
using Serilog;
using Shared.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Payment.Contracts.Commands.Forwards;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Providers;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Providers;

namespace WebApi.Services.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IRemoteTransactionManagerProvider _transactionManager;
        private readonly IRemoteWavesGatewayProvider _wavesGateway;
        private readonly IRemoteEventPublisher _eventPublisher;

        public DataContext DataContext { get; set; }
        public IUserIdentityService UserIdentityService { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        public AccountService(IRemoteTransactionManagerProvider transactionManager, IRemoteWavesGatewayProvider wavesGateway, IRemoteEventPublisher eventPublisher)
        {
            _transactionManager = transactionManager;
            _wavesGateway = wavesGateway;
            _eventPublisher = eventPublisher;
        }

        public async Task ActivateAsync(Network network, string userName)
        {
            if (network == Network.FREE)
            {
                throw new ArgumentException("Network not supported.");
            }

            var userAccount = DataContext.Accounts.OfType<UserAccount>().Single(x => x.Network == network && x.UserName == userName);
            if (userAccount.IsActive)
            {
                throw new ArgumentException("User account is active.");
            }

            var address = await _wavesGateway.Provide().Ask<AddressCreated>(new CreateAddress(network, userName, null));
            userAccount.DepositAddress = address.Address;
            userAccount.IsActive = true;
            await DataContext.SaveChangesAsync();

            _eventPublisher.Provide().Tell(new TriggerDeposit(new Identity(network, userName)));
        }

        public async Task<UserAccount> GetUserAccountAsync(Network network, string userName)
        {
            return await DataContext.Accounts
                .OfType<UserAccount>()
                .SingleAsync(x => x.Network == network && x.UserName == userName);
        }
        
        public async Task<GameAccount> GetGameAccountAsync(Network network, string userName)
        {
            return await DataContext.Accounts
                .OfType<GameAccount>()
                .SingleAsync(x => x.Network == network && x.UserName == userName);
        }

        public async Task<(string UserName, string Password)> CreateAsync()
        {
            var userName = UserIdentityService.CreateUserIdentity();
            var userPassword = SecurityHelper.CreateRandomPassword(10);

            try
            {
                var helper = new TransactionActorHelper(_transactionManager);
                helper.CreateAccount(Network.FREE, userName, 1000 * Money.Sathoshi, DateTime.UtcNow.Ticks);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Create Account {@ex}", ex);
                throw;
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                TwoFactorAuthSecret = SecurityHelper.CreateRandomPassword(8)
            };
            
            await UserManager.CreateAsync(user, userPassword);
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            await UserManager.ConfirmEmailAsync(user, token);
            await UserManager.AddToRoleAsync(user, AppRoles.GameUser);

            DataContext.Accounts.Add(new UserAccount
            {
                Network = Network.FREE,
                UserName = userName,
                DepositAddress = "",
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            });

            DataContext.Accounts.Add(new UserAccount
            {
                Network = Network.WAVES,
                UserName = userName,
                DepositAddress = "",
                UpdatedAt = DateTime.UtcNow,
                IsActive = false
            });

            DataContext.Accounts.Add(new UserAccount
            {
                Network = Network.GREEDYTEST,
                UserName = userName,
                DepositAddress = "",
                UpdatedAt = DateTime.UtcNow,
                IsActive = false
            });

            await DataContext.SaveChangesAsync();

            return (userName, userPassword);
        }

        public Task<long> CountAsync()
        {
            return DataContext.Accounts.OfType<UserAccount>().LongCountAsync();
        }

        
    }
}
