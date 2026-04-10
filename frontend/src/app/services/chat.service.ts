import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatMessage {
  role: 'user' | 'model';
  content: string;
}

export interface ChatResponse {
  reply: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private apiUrl = '/api/chat';

  constructor(private http: HttpClient) {}

  send(message: string, history: ChatMessage[]): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(this.apiUrl, { message, history });
  }
}
