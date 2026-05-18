import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

type Plan = 'pro' | 'team';

interface PlanDetails {
  name: string;
  emoji: string;
  color: string;
  features: string[];
}

@Component({
  selector: 'app-payment-success',
  imports: [CommonModule, RouterLink],
  templateUrl: './payment-success.html',
  styleUrl: './payment-success.scss'
})
export class PaymentSuccess implements OnInit {

  private readonly auth  = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  plan          = signal<Plan>('pro');
  animationStep = signal(0);
  refreshing    = signal(true);   // on attend le refresh avant d'afficher

  plans: Record<Plan, PlanDetails> = {
    pro: {
      name: 'Pro',
      emoji: '⚡',
      color: '#14151a',
      features: [
        '10 projets actifs débloqués',
        "Jusqu'à 2 000 feedbacks / mois",
        'Score de priorité IA activé',
        'Analyse de sentiment + topics clés',
        'Graphique de tendances 30 jours',
        'Export CSV disponible',
        'Réponse support sous 24h',
      ],
    },
    team: {
      name: 'Team',
      emoji: '🚀',
      color: '#14151a',
      features: [
        'Projets illimités',
        "Jusqu'à 10 000 feedbacks / mois",
        "Membres d'équipe illimités",
        'Gestion des rôles & permissions',
        'Tableau de bord partagé',
        'Réponse support sous 4h',
      ],
    },
  };

  currentPlan = computed(() => this.plans[this.plan()]);

  ngOnInit(): void {
    const planParam = this.route.snapshot.queryParamMap.get('plan');
    if (planParam === 'team') this.plan.set('team');

    // Rafraîchit le JWT pour que l'app connaisse le nouveau plan
    this.auth.refreshTokens().subscribe({
      next: () => this.startAnimation(),
      error: () => this.startAnimation(), // on affiche quand même en cas d'erreur
    });
  }

  private startAnimation(): void {
    this.refreshing.set(false);
    setTimeout(() => this.animationStep.set(1), 300);
    setTimeout(() => this.animationStep.set(2), 1000);
    setTimeout(() => this.animationStep.set(3), 1600);
  }
}