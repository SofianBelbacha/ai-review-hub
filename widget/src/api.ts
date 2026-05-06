export interface SubmitFeedbackPayload {
    content: string;
    projectToken: string;
    category?: string;
    pageUrl?: string;
    userAgent?: string;
}

export interface SubmitFeedbackResponse {
    id: string;
}

export async function submitFeedback(
    apiUrl: string,
    payload: SubmitFeedbackPayload
): Promise<SubmitFeedbackResponse> {
    const response = await fetch(`${apiUrl}/widget/feedback`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.message ?? `HTTP ${response.status}`);
    }

    return response.json();
}