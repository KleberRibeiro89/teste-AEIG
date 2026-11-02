import { http } from '../../lib/http.ts'

export interface DashboardStatus {
  nome: string
  quantidade: number
  valor: number
  valorPago: number
}

export interface DashboardMes {
  mes: string
  valor: number
}

export interface Dashboard {
  associados: number
  cobrancas: DashboardStatus[]
  recebidos: DashboardMes[]
  atrasos: DashboardMes[]
}

export function getDashboard(signal?: AbortSignal) {
  return http<Dashboard>('/api/Dashboard', { signal })
}
