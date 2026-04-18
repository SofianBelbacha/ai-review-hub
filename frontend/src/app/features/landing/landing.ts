import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

export interface FeatureCard {
  id: string;
  title: string;
  description: string;
  icon: string;
  color: string;
}


@Component({
  selector: 'app-landing',
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.html',
  styleUrl: './landing.scss',
})
export class Landing {

  // --- Hero tabs ---
  activeTab = signal<string>('Project');
 
  tabs = ['Project', 'Task', 'Message', 'Invoice', 'Clients', 'Timer'];
 
  setActiveTab(tab: string): void {
    this.activeTab.set(tab);
  }

    // --- Features ---
  featuresRow1: FeatureCard[] = [
    {
      id: 'ai-analysis',
      title: 'Analyse IA en temps réel',
      description: 'Chaque feedback est instantanément catégorisé et priorisé par l\'IA. Plus besoin de trier manuellement.',
      icon: 'brain',
      color: '#EEF2FF',
    },
    {
      id: 'kanban',
      title: 'Kanban visuel',
      description: 'Visualisez l\'état de tous vos retours en un coup d\'œil. Déplacez les cartes de "À traiter" à "Résolu".',
      icon: 'kanban',
      color: '#F0FDF4',
    },
  ];
 
  featuresRow2: FeatureCard[] = [
    {
      id: 'widget',
      title: 'Widget intégrable',
      description: 'Un simple snippet JS à coller sur le site de votre client. Aucun compte requis pour soumettre un retour.',
      icon: 'code',
      color: '#FFF7ED',
    },
    {
      id: 'projects',
      title: 'Multi-projets',
      description: 'Gérez plusieurs clients et projets depuis un seul tableau de bord centralisé.',
      icon: 'folder',
      color: '#F0F9FF',
    },
    {
      id: 'trends',
      title: 'Tendances & insights',
      description: 'Un graphique des 30 derniers jours pour détecter les pics de feedback et anticiper les problèmes.',
      icon: 'chart',
      color: '#FDF4FF',
    },
  ];


}
