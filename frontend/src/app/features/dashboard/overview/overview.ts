import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { DashboardData, OverviewService } from './overview.service';
import { UserService } from '../../../core/services/user.service';
import { RecentFeedback, TrendPoint } from './overview.types';
import { DatePipe } from '@angular/common';
import { interval, Subscription, switchMap, takeWhile } from 'rxjs';


@Component({
  selector: 'app-overview',
  imports: [DatePipe],
  templateUrl: './overview.html',
  styleUrl: './overview.scss',
})
export class Overview implements OnInit {
  private readonly overviewService = inject(OverviewService);
  private readonly userService = inject(UserService);
  private pollSubscription?: Subscription;


  // ─── State ────────────────────────────────────────────────
  loading = signal(true);
  error = signal('');
  data = signal<DashboardData | null>(null);

  // ─── Computed ─────────────────────────────────────────────
  readonly firstName = computed(() =>
    this.userService.profile()?.firstName ?? '');

  readonly stats = computed(() => this.data()?.stats ?? {
    totalFeedbacks: 0,
    todoCount: 0,
    inProgressCount: 0,
    resolvedCount: 0,
    highPriorityCount: 0,
  });

  readonly trends = computed(() => this.data()?.trends ?? []);
  readonly recentFeedbacks = computed(() => this.data()?.recentFeedbacks ?? []);

  // Feedbacks groupés par statut pour le kanban
  readonly todoFeedbacks = computed(() =>
    this.recentFeedbacks().filter(f => f.status === 'Todo').slice(0, 5));
  readonly inProgressFeedbacks = computed(() =>
    this.recentFeedbacks().filter(f => f.status === 'InProgress').slice(0, 5));
  readonly doneFeedbacks = computed(() =>
    this.recentFeedbacks().filter(f => f.status === 'Done').slice(0, 5));

  // ─── Graphique ────────────────────────────────────────────
  readonly maxTrendValue = computed(() => {
    const values = this.trends().map(t => t.count);
    return Math.max(...values, 1);
  });

  readonly trendBars = computed(() =>
    this.trends().map(t => ({
      ...t,
      height: Math.round((t.count / this.maxTrendValue()) * 100)
    }))
  );

  // ─── Lifecycle ────────────────────────────────────────────
  ngOnInit(): void {
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    this.pollSubscription?.unsubscribe();
  }


  loadDashboard(): void {
    this.loading.set(true);
    this.error.set('');

    this.overviewService.getDashboard().subscribe({
      next: (data) => {
        this.data.set(data);
        this.loading.set(false);
        this.startPollingIfNeeded();
      },
      error: () => {
        this.error.set('Impossible de charger le tableau de bord.');
        this.loading.set(false);
      }
    });
  }

  private startPollingIfNeeded(): void {
    const hasPending = this.recentFeedbacks().some(
      f => f.aiAnalysisStatus === 'Pending' ||
        f.aiAnalysisStatus === 'Processing'
    );

    if (!hasPending) {
      this.pollSubscription?.unsubscribe();
      return;
    }

    // Évite de créer plusieurs pollings
    if (this.pollSubscription) return;

    this.pollSubscription = interval(3000).pipe(
      switchMap(() => this.overviewService.getDashboard()),
      takeWhile(data =>
        data.recentFeedbacks.some(
          f => f.aiAnalysisStatus === 'Pending' ||
            f.aiAnalysisStatus === 'Processing'
        ), true // inclut le dernier emit
      )
    ).subscribe({
      next: (data) => {
        this.data.set(data);
        // Arrête le polling quand tout est analysé
        const stillPending = data.recentFeedbacks.some(
          f => f.aiAnalysisStatus === 'Pending' ||
            f.aiAnalysisStatus === 'Processing'
        );
        if (!stillPending) {
          this.pollSubscription?.unsubscribe();
          this.pollSubscription = undefined;
        }
      }
    });
  }
  // ─── Helpers ──────────────────────────────────────────────
  getCategoryLabel(category: string): string {
    const labels: Record<string, string> = {
      Bug: '🐛 Bug',
      FeatureRequest: '✨ Fonctionnalité',
      Question: '❓ Question',
      Uncategorized: '📝 Non catégorisé',
    };
    return labels[category] ?? category;
  }

  getPriorityClass(priority: string): string {
    const classes: Record<string, string> = {
      Critical: 'priority--critical',
      High: 'priority--high',
      Normal: 'priority--normal',
      Low: 'priority--low',
    };
    return classes[priority] ?? '';
  }

  trackByDate(_: number, item: TrendPoint): string {
    return item.date;
  }

  trackById(_: number, item: RecentFeedback): string {
    return item.id;
  }
}


