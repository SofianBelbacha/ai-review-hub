export type Period = 7 | 30 | 90;
export type ChartType = 'bar' | 'line';

export interface TrendPoint {
  date:  string;
  count: number;
}

export interface CategoryBreakdown {
  category:   string;
  count:      number;
  percentage: number;
}

export interface PriorityBreakdown {
  priority:   string;
  count:      number;
  percentage: number;
}

export interface TrendSummary {
  totalPeriod:   number;
  totalPrevious: number;
  growthRate:    number;
  avgPerDay:     number;
  peakCount:     number;
  peakDate:      string;
}

export interface TrendsData {
  dailyVolume:       TrendPoint[];
  categoryBreakdown: CategoryBreakdown[];
  priorityBreakdown: PriorityBreakdown[];
  summary:           TrendSummary;
}