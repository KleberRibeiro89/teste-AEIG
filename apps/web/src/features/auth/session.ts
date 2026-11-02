export const tokenKey = 'token'
const userKey = 'user'

export interface SessionUser {
  name: string
  email: string
}

export function readToken(): string | null {
  return localStorage.getItem(tokenKey)
}

export function readUser(): SessionUser | null {
  const raw = localStorage.getItem(userKey)
  if (!raw) return null
  try {
    const parsed = JSON.parse(raw) as SessionUser
    if (!parsed.name) return null
    return parsed
  } catch {
    return null
  }
}

export function saveSession(token: string, user: SessionUser) {
  localStorage.setItem(tokenKey, token)
  localStorage.setItem(userKey, JSON.stringify(user))
}

export function clearSession() {
  localStorage.removeItem(tokenKey)
  localStorage.removeItem(userKey)
}
