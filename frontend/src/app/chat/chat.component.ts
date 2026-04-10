import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService, ChatMessage } from '../services/chat.service';
import { MarkdownPipe } from './markdown.pipe';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownPipe],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {
  isOpen = false;
  userInput = '';
  messages: ChatMessage[] = [];
  loading = false;

  constructor(private chatService: ChatService) {}

  toggle(): void {
    this.isOpen = !this.isOpen;
  }

  send(): void {
    const text = this.userInput.trim();
    if (!text || this.loading) return;

    this.messages.push({ role: 'user', content: text });
    this.userInput = '';
    this.loading = true;

    this.chatService.send(text, this.messages.slice(0, -1)).subscribe({
      next: (res) => {
        this.messages.push({ role: 'model', content: res.reply });
        this.loading = false;
      },
      error: () => {
        this.messages.push({ role: 'model', content: 'Sorry, something went wrong. Try again.' });
        this.loading = false;
      }
    });
  }
}
