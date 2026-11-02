import { Navigate, Route, Routes } from 'react-router'
import { AssociadoDetalhePage } from './features/associados/AssociadoDetalhePage.tsx'
import { DashboardPage } from './features/dashboard/DashboardPage.tsx'
import { AssociadosPage } from './features/associados/AssociadosPage.tsx'
import { BaixaCobrancaPage } from './features/associados/BaixaCobrancaPage.tsx'
import { NovoAssociadoPage } from './features/associados/NovoAssociadoPage.tsx'
import { ParametroPagamentoPage } from './features/parametros/ParametroPagamentoPage.tsx'
import { NovoUsuarioPage } from './features/users/NovoUsuarioPage.tsx'
import { UsersPage } from './features/users/UsersPage.tsx'

export function AppRoutes() {
  return (
    <Routes>
      <Route index element={<DashboardPage />} />
      <Route path="/usuarios" element={<UsersPage />} />
      <Route path="/usuarios/novo" element={<NovoUsuarioPage />} />
      <Route path="/associados" element={<AssociadosPage />} />
      <Route path="/associados/novo" element={<NovoAssociadoPage />} />
      <Route path="/associados/:navigationId/cobrancas/:cobrancaId/baixa" element={<BaixaCobrancaPage />} />
      <Route path="/associados/:navigationId" element={<AssociadoDetalhePage />} />
      <Route path="/parametros" element={<ParametroPagamentoPage />} />
      <Route path="*" element={<Navigate to="/usuarios" replace />} />
    </Routes>
  )
}
