import { useEffect, useState } from 'react'
import { Navigate, Route, Routes } from 'react-router'
import { LoginPage } from './features/auth/LoginPage.tsx'
import { clearSession, readToken, readUser, saveSession, type SessionUser } from './features/auth/session.ts'
import { AssociadoDetalhePage } from './features/associados/AssociadoDetalhePage.tsx'
import { BaixaCobrancaPage } from './features/associados/BaixaCobrancaPage.tsx'
import { AssociadosPage } from './features/associados/AssociadosPage.tsx'
import { NovoAssociadoPage } from './features/associados/NovoAssociadoPage.tsx'
import { AppLayout } from './features/layout/AppLayout.tsx'
import { ParametroPagamentoPage } from './features/parametros/ParametroPagamentoPage.tsx'
import { NovoUsuarioPage } from './features/users/NovoUsuarioPage.tsx'
import { UsersPage } from './features/users/UsersPage.tsx'
import { http } from './lib/http.ts'

export default function App() {
  const [token, setToken] = useState(readToken)
  const [user, setUser] = useState<SessionUser | null>(readUser)

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
      <Routes>
        <Route index element={<Navigate to="/usuarios" replace />} />
        <Route path="/usuarios" element={<UsersPage />} />
        <Route path="/usuarios/novo" element={<NovoUsuarioPage />} />
        <Route path="/associados" element={<AssociadosPage />} />
        <Route path="/associados/novo" element={<NovoAssociadoPage />} />
        <Route path="/associados/:navigationId/cobrancas/:cobrancaId/baixa" element={<BaixaCobrancaPage />} />
        <Route path="/associados/:navigationId" element={<AssociadoDetalhePage />} />
        <Route path="/parametros" element={<ParametroPagamentoPage />} />
        <Route path="*" element={<Navigate to="/usuarios" replace />} />
      </Routes>
    </AppLayout>
  )
}
