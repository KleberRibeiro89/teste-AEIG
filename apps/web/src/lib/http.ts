import { unauthorizedMiddleware } from './unauthorized.ts'

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export function apiOrigin() {
  return (import.meta.env.VITE_API_URL ?? 'https://localhost:7115').replace(/\/$/, '')
}

function apiUrl(path: string): URL {
  return new URL(path.startsWith('/') ? path : `/${path}`, `${apiOrigin()}/`)
}

export async function http<T>(
  path: string,
  options: {
    method?: string
    body?: unknown
    query?: URLSearchParams
    signal?: AbortSignal
  } = {},
): Promise<T> {
  const url = apiUrl(path)
  options.query?.forEach((value, key) => url.searchParams.set(key, value))

  const headers = new Headers({ Accept: 'application/json' })
  const token = localStorage.getItem('token')
  if (token) headers.set('Authorization', `Bearer ${token}`)
  if (options.body !== undefined) headers.set('Content-Type', 'application/json')

  const response = await fetch(url, {
    method: options.method ?? 'GET',
    headers,
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
    signal: options.signal,
  })

  if (response.status === 204) return undefined as T

  unauthorizedMiddleware(path, response.status)

  if (!response.ok) {
    let message = response.statusText || 'Falha na requisição'
    try {
      const problem = (await response.json()) as {
        title?: string
        detail?: string
        errors?: Record<string, string[]>
      }
      const validation = problem.errors ? Object.values(problem.errors).flat()[0] : undefined
      message = validation || problem.detail || problem.title || message
    } catch {
      // resposta sem JSON
    }
    throw new ApiError(response.status, message)
  }

  return (await response.json()) as T
}
