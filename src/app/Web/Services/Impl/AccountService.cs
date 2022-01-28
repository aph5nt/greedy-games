using Akka.Actor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment.Messages.Commands.Transactions;
using Payment.Messages.Commands.Waves;
using Payment.Messages.Events.Waves;
using Persistance;
using Persistance.Model.Accounts;
using Shared.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Payment.Messages.Commands.Forwards;
using Web.Models;
using Web.Providers;

namespace Web.Services.Impl
{
    public class AccountService : IAccountService
    {
        private readonly ITransactionManagerProvider _transactionManager;
        private readonly IWavesGatewayProvider _wavesGateway;
        private readonly IEventPublisher _eventPublisher;

        public DataContext DataContext { get; set; }
        public IUserIdentityService UserIdentityService { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }

        public AccountService(ITransactionManagerProvider transactionManager, IWavesGatewayProvider wavesGateway, IEventPublisher eventPublisher)
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

        public async Task<UserAccount> GetAsync(Network network, string userName)
        {
            return await DataContext.Accounts
                .OfType<UserAccount>()
                .SingleAsync(x => x.Network == network && x.UserName == userName);
        }

        public async Task<(string userName, string password)> CreateAsync()
        {
            var userName = UserIdentityService.CreateUserIdentity();
            var userPassword = SHA256.Encode(Guid.NewGuid().ToString());

            var user = new ApplicationUser { UserName = userName };
            await UserManager.CreateAsync(user, userPassword);
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            await UserManager.ConfirmEmailAsync(user, token);
            await UserManager.AddToRoleAsync(user, "User");
            await UserManager.AddToRoleAsync(user, "CanUseHubs");
            
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
                Network = Network.WAVESTEST,
                UserName = userName,
                DepositAddress = "",
                UpdatedAt = DateTime.UtcNow,
                IsActive = false
            });

            await DataContext.SaveChangesAsync();

            var helper = new TransactionActorSelectionHelper(_transactionManager.Provide());
            helper.CreateAccount(Network.FREE, userName, 1000 * Money.Sathoshi, DateTime.UtcNow.Ticks);

            return (userName, userPassword);
        }
    }
}
