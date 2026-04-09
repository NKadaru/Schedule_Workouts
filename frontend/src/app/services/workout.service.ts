import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Workout } from '../models/workout';

@Injectable({ providedIn: 'root' })
export class WorkoutService {
  private apiUrl = '/api/workouts';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Record<string, Workout[]>> {
    return this.http.get<Record<string, Workout[]>>(this.apiUrl);
  }

  getByDay(day: string): Observable<Workout[]> {
    return this.http.get<Workout[]>(`${this.apiUrl}/${day}`);
  }
}
