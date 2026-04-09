import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Workout } from '../models/workout';

export interface DayPlan {
  quote: string;
  exercises: Workout[];
}

@Injectable({ providedIn: 'root' })
export class WorkoutService {
  private apiUrl = '/api/workouts';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Record<string, DayPlan>> {
    return this.http.get<Record<string, DayPlan>>(this.apiUrl);
  }
}
