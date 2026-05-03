import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DashboardStats, TrendPoint, RecentFeedback } from './overview.types';

export interface DashboardData {
  stats: DashboardStats;
  trends: TrendPoint[];
  recentFeedbacks: RecentFeedback[];
}

@Injectable({ providedIn: 'root' })
export class OverviewService {
  private readonly http = inject(HttpClient);
  private readonly API  = environment.apiUrl;

  getDashboard(projectId?: string): Observable<DashboardData> {
    const params = projectId ? `?projectId=${projectId}` : '';
    return this.http.get<DashboardData>(
      `${this.API}/dashboard${params}`,
      { withCredentials: true }
    );
  }
}