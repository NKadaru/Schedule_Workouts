import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthService } from './services/health.service';
import { HealthResponse } from './models/health-response';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'frontend';
  healthResponse: HealthResponse | null = null;
  errorMessage: string | null = null;

  constructor(private healthService: HealthService) {}

  ngOnInit(): void {
    this.healthService.checkHealth().subscribe({
      next: (response) => {
        this.healthResponse = response;
        this.errorMessage = null;
      },
      error: () => {
        this.healthResponse = null;
        this.errorMessage = 'Unable to connect to the API server';
      }
    });
  }
}
