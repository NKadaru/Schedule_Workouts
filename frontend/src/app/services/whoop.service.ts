import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DailyEntry {
  date: string;
  value: number;
  level: string;
}

export interface DailySummary {
  date: string;
  recovery: number | null;
  recoveryLevel: string;
  strain: number | null;
  sleepHours: number | null;
}

export interface WhoopDashboard {
  recoveryScore: number | null;
  restingHeartRate: number | null;
  hrv: number | null;
  strain: number | null;
  sleepPerformance: number | null;
  sleepHours: number | null;
  recoveryLevel: string;
  monthlyRecovery: DailyEntry[];
  monthlyStrain: DailyEntry[];
  monthlySleep: DailyEntry[];
  dailyHistory: DailySummary[];
}

@Injectable({ providedIn: 'root' })
export class WhoopService {
  constructor(private http: HttpClient) {}

  getAuthorizeUrl(): Observable<{ url: string }> {
    return this.http.get<{ url: string }>('/api/whoop/authorize');
  }

  getStatus(): Observable<{ connected: boolean }> {
    return this.http.get<{ connected: boolean }>('/api/whoop/status');
  }

  getDashboard(): Observable<WhoopDashboard> {
    return this.http.get<WhoopDashboard>('/api/whoop/dashboard');
  }
}
