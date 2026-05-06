export interface Project {
  id:           string;
  name:         string;
  description:  string;
  publicToken:  string;
  isActive:     boolean;
  feedbackCount: number;
  createdAt:    string;
}

export interface CreateProjectRequest {
  name:        string;
  description: string;
}