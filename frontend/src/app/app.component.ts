import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthService } from './services/health.service';
import { WorkoutService, DayPlan } from './services/workout.service';
import { HealthResponse } from './models/health-response';
import { Workout } from './models/workout';
import { ChatComponent } from './chat/chat.component';
import { WhoopComponent } from './whoop/whoop.component';

const FALLBACK_DATA: Record<string, DayPlan> = {
  Monday: {
    quote: "New week, new goals. Let's crush it! 💪",
    exercises: [
      { name: 'Barbell Squats', sets: 4, reps: 8, notes: 'Full body compound - legs focus' },
      { name: 'Bench Press', sets: 4, reps: 8, notes: 'Flat barbell - chest/triceps' },
      { name: 'Barbell Rows', sets: 4, reps: 10, notes: 'Overhand grip - back/biceps' },
      { name: 'Overhead Press', sets: 3, reps: 10, notes: 'Standing - shoulders/core' },
      { name: 'Romanian Deadlifts', sets: 3, reps: 10, notes: 'Hamstrings/glutes/lower back' },
      { name: 'Plank', sets: 3, reps: 0, notes: 'Hold 45 seconds - core stability' }
    ]
  },
  Tuesday: {
    quote: "Push yourself, because no one else is going to do it for you. 🔥",
    exercises: [
      { name: 'Deadlifts', sets: 4, reps: 6, notes: 'Conventional - full posterior chain' },
      { name: 'Incline Dumbbell Press', sets: 4, reps: 10, notes: 'Upper chest/shoulders' },
      { name: 'Pull-ups', sets: 4, reps: 8, notes: 'Back/biceps - add weight if possible' },
      { name: 'Lunges', sets: 3, reps: 12, notes: 'Alternating legs - quads/glutes' },
      { name: 'Dumbbell Lateral Raises', sets: 3, reps: 15, notes: 'Light weight - side delts' },
      { name: 'Hanging Leg Raises', sets: 3, reps: 12, notes: 'Lower abs/hip flexors' }
    ]
  },
  Wednesday: {
    quote: "Halfway through the week — keep the momentum going! 🏋️",
    exercises: [
      { name: 'AMRAP 25 min — Functional Fitness', sets: 0, reps: 0, notes: 'As many rounds as possible in 25 minutes' },
      { name: 'Wall Balls', sets: 1, reps: 15, notes: '20/14 lb medicine ball to 10/9 ft target' },
      { name: 'Box Jumps', sets: 1, reps: 12, notes: '24/20 inch box — step down' },
      { name: 'Kettlebell Swings', sets: 1, reps: 15, notes: '53/35 lb — Russian style' },
      { name: 'Burpees', sets: 1, reps: 10, notes: 'Chest to floor' },
      { name: 'Rowing', sets: 1, reps: 0, notes: '250m row between rounds' }
    ]
  },
  Thursday: {
    quote: "Your body can stand almost anything. It's your mind you have to convince. 🧠",
    exercises: [
      { name: 'For Time — 25 min cap — Functional Fitness', sets: 0, reps: 0, notes: 'Complete all rounds as fast as possible' },
      { name: 'Thrusters', sets: 5, reps: 10, notes: '95/65 lb barbell — front squat to press' },
      { name: 'Toes-to-Bar', sets: 5, reps: 10, notes: 'Hanging from pull-up bar' },
      { name: 'Power Cleans', sets: 5, reps: 8, notes: '115/75 lb — explosive hip drive' },
      { name: 'Double Unders', sets: 5, reps: 30, notes: 'Sub 60 single unders if needed' },
      { name: 'Assault Bike', sets: 5, reps: 0, notes: '15 cal between rounds' }
    ]
  },
  Friday: {
    quote: "Finish the week strong. You've earned it! ⚡",
    exercises: [
      { name: '3-Mile Run', sets: 1, reps: 0, notes: 'Steady pace — aim for 24-27 min' },
      { name: 'Warm-up Jog', sets: 1, reps: 0, notes: '5 min easy pace before run' },
      { name: 'Cooldown Walk', sets: 1, reps: 0, notes: '5 min walk + stretching after run' }
    ]
  },
  Saturday: {
    quote: "Weekends are for warriors. Get after it! 🦾",
    exercises: [
      { name: 'Bicep Curls', sets: 3, reps: 12, notes: 'Dumbbell' },
      { name: 'Tricep Dips', sets: 3, reps: 12, notes: '' },
      { name: 'Plank', sets: 3, reps: 0, notes: 'Hold 60 seconds each' }
    ]
  },
  Sunday: {
    quote: "Rest, recover, and come back stronger tomorrow. 😌",
    exercises: [
      { name: 'Rest Day', sets: 0, reps: 0, notes: 'Full rest' }
    ]
  }
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ChatComponent, WhoopComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'GrindFlow';
  healthResponse: HealthResponse | null = null;
  errorMessage: string | null = null;

  days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  selectedDay = 'Monday';
  dayPlans: Record<string, DayPlan> = {};
  loading = true;
  completed: Record<string, Set<number>> = {};

  activeView: 'schedule' | 'whoop' = 'schedule';

  constructor(
    private healthService: HealthService,
    private workoutService: WorkoutService
  ) {}

  ngOnInit(): void {
    this.selectedDay = this.days[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];

    this.healthService.checkHealth().subscribe({
      next: (response) => {
        this.healthResponse = response;
        this.errorMessage = null;
      },
      error: () => {
        this.healthResponse = null;
        this.errorMessage = 'Unable to connect to the API server — using offline data';
      }
    });

    this.workoutService.getAll().subscribe({
      next: (data) => {
        this.dayPlans = data;
        this.days.forEach(d => this.completed[d] = new Set());
        this.loading = false;
      },
      error: () => {
        this.dayPlans = FALLBACK_DATA;
        this.days.forEach(d => this.completed[d] = new Set());
        this.loading = false;
      }
    });
  }

  selectDay(day: string): void {
    this.selectedDay = day;
  }

  get currentWorkouts(): Workout[] {
    return this.dayPlans[this.selectedDay]?.exercises || [];
  }

  get currentQuote(): string {
    return this.dayPlans[this.selectedDay]?.quote || '';
  }

  toggleComplete(index: number): void {
    const set = this.completed[this.selectedDay];
    set.has(index) ? set.delete(index) : set.add(index);
  }

  isComplete(index: number): boolean {
    return this.completed[this.selectedDay]?.has(index) ?? false;
  }

  get completedCount(): number {
    return this.completed[this.selectedDay]?.size ?? 0;
  }

  get progressPercent(): number {
    const total = this.currentWorkouts.length;
    return total === 0 ? 0 : Math.round((this.completedCount / total) * 100);
  }
}
