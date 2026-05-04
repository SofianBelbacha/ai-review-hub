export interface DashboardStats {
  totalFeedbacks: number;
  todoCount: number;
  inProgressCount: number;
  resolvedCount: number;
  highPriorityCount: number;
}

export interface TrendPoint {
  date: string;
  count: number;
}

export interface RecentFeedback {
  id: string;
  content: string;
  aiSummary: string;
  category: string;
  priority: string;
  status: string;
  aiAnalysisStatus: 'Pending' | 'Processing' | 'Completed' | 'Failed';
  createdAt: string;
}