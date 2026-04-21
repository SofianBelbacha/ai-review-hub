import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Footer } from '../../shared/components/footer/footer';

export interface FeatureCard {
  id: string;
  title: string;
  description: string;
  image: string;
  color: string;
}

export interface WorkflowCard {
  id: string;
  title: string;
  description: string;
  icon: string;
}

export interface PricingPlan {
  id: string;
  name: string;
  price: number | null;
  priceLabel?: string;
  description: string;
  cta: string;
  ctaPath: string;
  featured: boolean;
  features: string[];
}

export interface FaqItem {
  id: string;
  question: string;
  answer: string;
}


@Component({
  selector: 'app-landing',
  imports: [CommonModule, RouterLink, Footer],
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
      image: 'image/project_manager.png',
      color: '#EEF2FF',
    },
    {
      id: 'kanban',
      title: 'Kanban visuel',
      description: 'Visualisez l\'état de tous vos retours en un coup d\'œil. Déplacez les cartes de "À traiter" à "Résolu".',
      image: 'image/collaboration.png',
      color: '#F0FDF4',
    },
  ];
 
  featuresRow2: FeatureCard[] = [
    {
      id: 'widget',
      title: 'Widget intégrable',
      description: 'Un simple snippet JS à coller sur le site de votre client. Aucun compte requis pour soumettre un retour.',
      image: 'image/track.png',
      color: '#FFF7ED',
    },
    {
      id: 'projects',
      title: 'Multi-projets',
      description: 'Gérez plusieurs clients et projets depuis un seul tableau de bord centralisé.',
      image: 'image/integration.png',
      color: '#F0F9FF',
    },
    {
      id: 'trends',
      title: 'Tendances & insights',
      description: 'Un graphique des 30 derniers jours pour détecter les pics de feedback et anticiper les problèmes.',
      image: 'image/trends.png',
      color: '#FDF4FF',
    },
  ];

  // --- Workflow — grille de mini-cartes icône + texte ---
  workflowRow1: WorkflowCard[] = [
    {
      id: 'clarity',
      title: 'Clarté & responsabilité',
      description: 'Chaque retour est attribué, tracé et visible. Fini les feedbacks perdus dans les emails.',
      icon: 'database',
    },
    {
      id: 'insights',
      title: 'Insights orientés données',
      description: 'Détectez les tendances, mesurez les volumes et anticipez les problèmes avant qu\'ils s\'aggravent.',
      icon: 'trend',
    },
    {
      id: 'priority',
      title: 'Priorisation intelligente',
      description: 'Le score de priorité calculé par l\'IA vous indique immédiatement ce qu\'il faut traiter en premier.',
      icon: 'clock',
    },
  ];
 
  workflowRow2: WorkflowCard[] = [
    {
      id: 'automation',
      title: 'Traitement automatisé',
      description: 'L\'IA catégorise et résume chaque feedback à votre place. Concentrez-vous sur l\'essentiel.',
      icon: 'settings',
    },
    {
      id: 'collaboration',
      title: 'Collaboration en équipe',
      description: 'Invitez vos collègues, partagez les projets et avancez ensemble sur les retours clients.',
      icon: 'users',
    },
    {
      id: 'freemium',
      title: 'Modèle freemium simple',
      description: 'Commencez gratuitement, passez Pro quand vous en avez besoin. Sans engagement.',
      icon: 'invoice',
    },
  ];

  // -----------------------------------------------
  // Pricing
  // -----------------------------------------------
  plans: PricingPlan[] = [
    {
      id: 'free',
      name: 'Free',
      price: 0,
      description: 'Pour tester la valeur du produit sans engagement.',
      cta: 'Commencer gratuitement',
      ctaPath: '/register',
      featured: false,
      features: [
        '1 projet actif',
        '50 feedbacks / mois',
        'Analyse IA (catégorie + résumé)',
        'Tableau kanban',
        'Widget intégrable',
        'Support par email',
      ],
    },
    {
      id: 'pro',
      name: 'Pro',
      price: 9,
      description: 'Pour les freelances et indépendants qui gèrent plusieurs clients.',
      cta: 'Passer au Pro',
      ctaPath: '/register?plan=pro',
      featured: true,
      features: [
        '10 projets actifs',
        'Feedbacks illimités',
        'Analyse IA complète + score de priorité',
        'Tableau kanban + filtres avancés',
        'Widget intégrable personnalisable',
        'Graphique de tendances 30 jours',
        'Export CSV',
        'Support prioritaire',
      ],
    },
    {
      id: 'team',
      name: 'Team',
      price: 29,
      description: 'Pour les agences qui travaillent en équipe sur les mêmes projets.',
      cta: 'Contacter l\'équipe',
      ctaPath: '/contact',
      featured: false,
      features: [
        'Projets illimités',
        'Feedbacks illimités',
        'Tout le plan Pro',
        'Membres d\'équipe illimités',
        'Gestion des rôles & permissions',
        'Tableau de bord partagé',
        'Intégrations (Slack, Notion…)',
        'Support dédié + onboarding',
      ],
    },
  ];


  // -----------------------------------------------
  // FAQ — état d'ouverture géré avec un signal
  // -----------------------------------------------
  openFaqId = signal<string | null>(null);
 
  toggleFaq(id: string): void {
    this.openFaqId.update(current => current === id ? null : id);
  }
 
  faqs: FaqItem[] = [
    {
      id: 'faq-1',
      question: 'Comment fonctionne l\'analyse IA des feedbacks ?',
      answer: 'Dès qu\'un retour est soumis via le widget ou le lien public, notre backend l\'envoie à l\'API OpenAI. En quelques secondes, l\'IA retourne trois informations : la catégorie du retour (bug, feature request ou question), un résumé en une phrase claire, et un score de priorité calculé à partir de l\'analyse du sentiment. Ces données enrichissent automatiquement votre kanban.',
    },
    {
      id: 'faq-2',
      question: 'Mes clients ont-ils besoin de créer un compte pour soumettre un retour ?',
      answer: 'Non, c\'est l\'un de nos partis pris forts. Votre client accède à un lien public ou interagit avec un widget JavaScript intégré directement sur son site. Il remplit un simple formulaire et soumet son retour — sans inscription, sans mot de passe, sans friction.',
    },
    {
      id: 'faq-3',
      question: 'Quelle est la différence entre le plan Free et le plan Pro ?',
      answer: 'Le plan Free autorise 1 projet actif et jusqu\'à 50 feedbacks par mois, ce qui est suffisant pour tester la plateforme avec un premier client. Le plan Pro (9 €/mois) débloque 10 projets, les feedbacks illimités, les filtres avancés, l\'export CSV et le graphique de tendances. Le plan Team ajoute la gestion multi-membres et les intégrations tierces.',
    },
    {
      id: 'faq-4',
      question: 'Puis-je annuler mon abonnement à tout moment ?',
      answer: 'Oui, sans engagement ni frais cachés. Vous pouvez annuler depuis votre espace compte en un clic. Votre accès Pro reste actif jusqu\'à la fin de la période déjà facturée, puis bascule automatiquement sur le plan Free.',
    },
    {
      id: 'faq-5',
      question: 'Le widget est-il compatible avec tous les types de sites ?',
      answer: 'Oui. Le widget est un simple snippet JavaScript universel — il fonctionne sur n\'importe quel site web, qu\'il soit construit avec WordPress, Webflow, un framework React ou Vue, ou même du HTML statique. L\'intégration prend moins de deux minutes.',
    },
    {
      id: 'faq-6',
      question: 'Mes données sont-elles sécurisées ?',
      answer: 'Les données sont hébergées sur une infrastructure cloud certifiée (Railway / Supabase) avec chiffrement en transit (TLS) et au repos. L\'authentification utilise un système JWT avec refresh tokens. Nous ne revendons jamais vos données ni celles de vos clients à des tiers.',
    },
  ];


}
