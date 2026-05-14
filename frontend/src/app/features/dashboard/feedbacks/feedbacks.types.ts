export type FeedbackStatus = 'Todo' | 'InProgress' | 'Done';
export type FeedbackPriority = 'Low' | 'Normal' | 'High' | 'Critical';
export type FeedbackCategory = 'Bug' | 'FeatureRequest' | 'Question' | 'Uncategorized';
export type AiStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed';

export interface Feedback {
    id:               string;
    content:          string;
    aiSummary:        string;
    category:         FeedbackCategory;
    priority:         FeedbackPriority;
    status:           FeedbackStatus;
    aiAnalysisStatus: AiStatus;
    // Champs Pro
    priorityScore?:   number;
    sentiment?:       string;
    sentimentScore?:  number;
    keyTopics?:       string[];
    actionRequired?:  boolean;
    urgency?:         string;
    createdAt:        string;
    updatedAt?:       string;
}

export interface FeedbackFilters {
    category?: FeedbackCategory;
    priority?: FeedbackPriority;
    search: string;
    page: number;
    pageSize: number;
    status?: FeedbackStatus;
}

export interface PagedResult<T> {
    data: T[];
    meta: {
        total: number;
        page: number;
        pageSize: number;
        totalPages: number;
    };
}