/** API error with its status, message and field errors. */
export class ApiError extends Error {
  status: number
  errors: string[]

  constructor(status: number, message: string, errors: string[] = []) {
    super(message)
    this.status = status
    this.errors = errors
  }
}

/** Calls the API; throws ApiError on failure. The browser sends the login cookie itself. */
export async function api<T>(method: string, url: string, body?: unknown): Promise<T> {
  const response = await fetch(`/api${url}`, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  if (!response.ok) {
    const error = await response.json().catch(() => ({}))
    throw new ApiError(response.status, error.message ?? 'Something went wrong. Please try again.', error.errors ?? [])
  }

  return response.status === 204 ? (undefined as T) : response.json()
}
