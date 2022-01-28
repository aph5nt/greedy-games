import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthService, AuthState } from '../../../services/auth.service';
import { OnDestroy } from '@angular/core/src/metadata/lifecycle_hooks';
import { HubConnection } from '@aspnet/signalr-client';
import { environment } from '../../../../environments/environment'
import { forEach } from '@angular/router/src/utils/collection';


export class ChatMessage {
  constructor(public userName: string, public message: string) {
  }
}

export class ChatArchive {
  messages: ChatMessage[]
}

export class UsersOnline {
  count: number;
}

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {


  authState: AuthState = AuthState.Empty();

  onlineUsers = 0;
  messages: Array<ChatMessage>;
  userName: string;
  message = '';
  hubConnection: HubConnection;

  constructor(public authService: AuthService, private change: ChangeDetectorRef) { }

  ngOnInit() {
    this.authService.AuthState.subscribe(state => {
      this.authState = state;

      if (state.isLogged) {

        this.messages = [];
        this.userName = state.userName;
        this.change.detectChanges();

        this.hubConnection = new HubConnection(`${environment.apiBaseUrl}/chat?hubToken=${this.authService.getAuth().token}`);
         
        this.hubConnection.on('onUsersOnlineNumberChange', (data: UsersOnline) => {
          this.onlineUsers = data.count;
          this.change.detectChanges();
        });

        this.hubConnection.on('onArchiveMessagesSent', (data: ChatArchive) => {
          data.messages.forEach(i => {
            this.messages.push(new ChatMessage(i.userName, i.message));
          })

          this.change.detectChanges();
        });

        this.hubConnection.on('onMessageSent', (chatMessage: ChatMessage) => {
          this.messages.push(chatMessage);
          this.change.detectChanges();
        });

        this.hubConnection
          .start()
          .then(() => console.log('Connection started!'))
          .catch(err => console.log('Error while establishing connection :('));
      }

      else {
        this.onlineUsers = 0;
        this.messages = [];
        this.change.detectChanges();

        if(this.hubConnection) {
          this.hubConnection.stop();
        }
      }

    });
  }

  ngOnDestroy(): void {
    this.hubConnection.stop();
  }

  getUserName(message: ChatMessage): string {
    if (message.userName === this.userName) {
      return 'YOU';
    }
    return message.userName;
  }

  toggleSendMessage() {

    this.hubConnection.send("sendMessage", this.message);
    this.message = '';
  }

  watchScroll() {
    const chat = document.getElementById('chat');
    setInterval(() => {
      // allow 1px inaccuracy by adding 1

      const isScrolledToBottom = chat.scrollHeight - chat.clientHeight <= chat.scrollTop + 26;

      console.log(chat.scrollHeight - chat.clientHeight, chat.scrollTop);
      console.log(isScrolledToBottom);

      if (isScrolledToBottom) {
        chat.scrollTop = chat.scrollHeight - chat.clientHeight + 20;
      }
    }, 1000);
  }

}
