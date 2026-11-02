import { http } from '../../lib/http.ts'

export interface DashboardStatus {
  nome: string
  quantidade: number
  valor: number
  valorPago: number
}

export interface Dashboard {
  associados: number
  cobrancas: DashboardStatus[]
}

export function getDashboard(signal?: AbortSignal) {
  return http<Dashboard>('/api/Dashboard', { signal })
}
