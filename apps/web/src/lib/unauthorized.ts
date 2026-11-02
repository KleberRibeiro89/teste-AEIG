import { clearSession } from '../features/auth/session.ts'

const loginPath = '/api/Auth/login'

type UnauthorizedHandler = () => void

let onUnauthorized: UnauthorizedHandler | null = null

export function setUnauthorizedHandler(handler: UnauthorizedHandler | null) {
  onUnauthorized = handler
}

export function unauthorizedMiddleware(path: string, status: number) {
  if (status !== 401 || path === loginPath) return
  clearSession()
  onUnauthorized?.()
}
