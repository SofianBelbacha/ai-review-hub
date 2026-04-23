import { AfterViewInit, Component, ElementRef, OnDestroy, QueryList, signal, ViewChildren } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Footer } from '../../shared/components/footer/footer';
import { Navbar } from '../../shared/components/navbar/navbar';


export interface ProblemCard {
  id: string;
  title: string;
  body: string;
  quote: string;
  icon: string;
  accent: 'red' | 'amber';
}

export interface SolutionStep {
  id: string;
  number: string;
  title: string;
  description: string;
  icon: string;
  details: string[];
}

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
  imports: [CommonModule, RouterLink, Navbar, Footer],
  templateUrl: './landing.html',
  styleUrl: './landing.scss',
})
export class Landing implements AfterViewInit, OnDestroy {

  // --- Hero tabs ---
  activeTab = signal<string>('Project');
 
  tabs = ['Project', 'Task', 'Message', 'Invoice', 'Clients', 'Timer'];
 
  setActiveTab(tab: string): void {
    this.activeTab.set(tab);
  }

  // -----------------------------------------------
  // Problem section — animation au scroll
  // -----------------------------------------------
  @ViewChildren('problemCard') problemCardRefs!: QueryList<ElementRef>;
  private observer?: IntersectionObserver;
 
  problemCards: ProblemCard[] = [
    {
      id: 'p1',
      title: 'Les retours arrivent de partout',
      body: 'Email, Slack, Notion, appels Zoom, post-it… Aucun endroit unique. Vous cherchez, vous scrollez, vous oubliez.',
      quote: '« J\'ai trouvé un bug critique dans un fil Slack de 3 semaines. »',
      icon: 'chat',
      accent: 'red',
    },
    {
      id: 'p2',
      title: 'Impossible de savoir quoi traiter en premier',
      body: 'Bug bloquant ou demande cosmétique ? Sans priorisation claire, on commence par ce qui est facile, pas urgent.',
      quote: '« On a corrigé la couleur d\'un bouton pendant qu\'un client ne pouvait pas payer. »',
      icon: 'clock',
      accent: 'amber',
    },
    {
      id: 'p3',
      title: 'Des retours qui se perdent sans réponse',
      body: 'Le client a signalé un problème. Il attend. Personne ne l\'a vu. Il relance. Vous perdez sa confiance avant d\'avoir corrigé quoi que ce soit.',
      quote: '« Mon client pensait qu\'on ignorait ses retours. »',
      icon: 'alert',
      accent: 'red',
    },
    {
      id: 'p4',
      title: 'Le tri prend des heures chaque semaine',
      body: 'Lire, comprendre, reformuler, catégoriser, répondre. Pour chaque feedback. Multiplié par dix clients. C\'est un mi-temps que personne n\'a payé.',
      quote: '« Je passe le lundi matin entier à trier des emails avant de coder. »',
      icon: 'users',
      accent: 'amber',
    },
    {
      id: 'p5',
      title: 'Aucune visibilité sur les tendances',
      body: 'Le même problème revient chaque semaine, mais personne ne l\'a détecté parce que les retours sont éparpillés. Un graphique aurait suffi.',
      quote: '« On a découvert le bug 3 mois après. 4 clients l\'avaient signalé. »',
      icon: 'chart',
      accent: 'red',
    },
    {
      id: 'p6',
      title: 'La phase de recettage est toujours chaotique',
      body: 'À la livraison d\'un projet, les retours explosent. Sans système, c\'est le chaos. Chacun gère à sa façon, rien n\'est tracé.',
      quote: '« La recette, c\'est le moment où tout part en vrille. »',
      icon: 'grid',
      accent: 'amber',
    },
  ];
 
  ngAfterViewInit(): void {
    this.observer = new IntersectionObserver(
      entries => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            (entry.target as HTMLElement).classList.add('problem-card--visible');
            this.observer?.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12 }
    );
 
    this.problemCardRefs.forEach(ref => this.observer?.observe(ref.nativeElement));
  }
 
  ngOnDestroy(): void {
    this.observer?.disconnect();
  }

  // Solution section 
  solutionSteps: SolutionStep[] = [
    {
      id: 's1',
      number: '01',
      title: 'Collectez sans friction',
      description: 'Vos clients soumettent leurs retours via un widget intégré ou un lien public. Aucun compte requis. Aucune friction.',
      icon: 'widget',
      details: [
        'Widget JS en 2 lignes de code',
        'Lien public partageable',
        'Formulaire simple, sans inscription',
      ],
    },
    {
      id: 's2',
      number: '02',
      title: 'L\'IA trie et priorise',
      description: 'Chaque retour est automatiquement catégorisé (bug, feature, question), résumé en une phrase et scoré selon le sentiment détecté.',
      icon: 'ai',
      details: [
        'Catégorisation automatique',
        'Résumé IA en une phrase',
        'Score de priorité par sentiment',
      ],
    },
    {
      id: 's3',
      number: '03',
      title: 'Agissez sur ce qui compte',
      description: 'Votre kanban affiche les retours priorisés. Vous traitez en premier ce qui bloque vraiment, pas ce qui est arrivé en dernier.',
      icon: 'kanban',
      details: [
        'Kanban visuel par projet',
        'Filtres par catégorie et priorité',
        'Graphique de tendances 30 jours',
      ],
    },
  ];

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
