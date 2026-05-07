import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TrendsData } from './trends.types';

@Injectable({ providedIn: 'root' })
export class TrendsService {
  private readonly http = inject(HttpClient);
  private readonly API  = environment.apiUrl;

  get(days: number, projectId?: string): Observable<TrendsData> {
    let params = new HttpParams().set('days', days);
    if (projectId) params = params.set('projectId', projectId);

    return this.http.get<TrendsData>(
      `${this.API}/trends`,
      { params, withCredentials: true }
    );
  }
}