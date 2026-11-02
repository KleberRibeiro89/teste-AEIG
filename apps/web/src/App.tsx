import { useEffect, useState } from 'react'
import { LoginPage } from './features/auth/LoginPage.tsx'
import { clearSession, readToken, readUser, saveSession, type SessionUser } from './features/auth/session.ts'
import { AppLayout } from './features/layout/AppLayout.tsx'
import { http } from './lib/http.ts'
import { setUnauthorizedHandler } from './lib/unauthorized.ts'
import { AppRoutes } from './AppRoutes.tsx'
import { CobrancaNotifications } from './features/cobrancas/CobrancaNotifications.tsx'

export default function App() {
  const [token, setToken] = useState(readToken)
  const [user, setUser] = useState<SessionUser | null>(readUser)

  useEffect(() => {
    setUnauthorizedHandler(() => {
      setUser(null)
      setToken(null)
    })
    return () => setUnauthorizedHandler(null)
  }, [])

  useEffect(() => {
    if (!token || user?.name) return
    const controller = new AbortController()
    http<SessionUser>('/api/Auth/me', { signal: controller.signal })
      .then((me) => {
        if (!me.name) return
        const next = { name: me.name, email: me.email }
        localStorage.setItem('user', JSON.stringify(next))
        setUser(next)
      })
      .catch(() => {})
    return () => controller.abort()
  }, [token, user])

  if (!token) {
    return (
      <LoginPage
        onSuccess={(result) => {
          const next = { name: result.user.nome, email: result.user.email }
          saveSession(result.token, next)
          setUser(next)
          setToken(result.token)
        }}
      />
    )
  }

  return (
    <AppLayout
      userName={user?.name ?? ''}
      onLogout={() => {
        clearSession()
        setUser(null)
        setToken(null)
      }}
    >
      <CobrancaNotifications />
      <AppRoutes />
    </AppLayout>
  )
}
