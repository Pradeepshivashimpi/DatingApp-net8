import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_Services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [ButtonsModule, FormsModule, TimeagoModule],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent implements OnInit {
   messageService = inject(MessageService);
   container = 'Inbox';
   pageNumber = 1;
   pageSize = 5;

   ngOnInit(): void {
     this.loadMessages();
   }

   loadMessages() {
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container);
   }

   pageChanged(event: any) {
    if(this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
   }
}
