import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthService } from './services/health.service';
import { WorkoutService } from './services/workout.service';
import { HealthResponse } from './models/health-response';
import { Workout } from './models/workout';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Workout Scheduler';
  healthResponse: HealthResponse | null = null;
  errorMessage: string | null = null;

  days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  selectedDay = 'Monday';
  workouts: Record<string, Workout[]> = {};
  loading = true;

  constructor(
    private healthService: HealthService,
    private workoutService: WorkoutService
  ) {}

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

    this.workoutService.getAll().subscribe({
      next: (data) => {
        this.workouts = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  selectDay(day: string): void {
    this.selectedDay = day;
  }

  get currentWorkouts(): Workout[] {
    return this.workouts[this.selectedDay] || [];
  }
}
