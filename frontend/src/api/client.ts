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

let lastSuccessAt = Date.now()

/** When the API last answered; each answer keeps the session alive. */
export const lastApiSuccessAt = () => lastSuccessAt

/** Calls the API; throws ApiError on failure. The browser sends the login cookie itself. */
export async function api<T>(method: string, url: string, body?: unknown): Promise<T> {
  const response = await fetch(`/api${url}`, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })

  // Session ended elsewhere (another device signed this one out): back to login
  if (response.status === 401 && !url.startsWith('/auth/')) {
    window.location.assign('/login?signedOut=1')
  }

  if (!response.ok) {
    const error = await response.json().catch(() => ({}))
    throw new ApiError(response.status, error.message ?? 'Something went wrong. Please try again.', error.errors ?? [])
  }

  lastSuccessAt = Date.now()
  return response.status === 204 ? (undefined as T) : response.json()
}
