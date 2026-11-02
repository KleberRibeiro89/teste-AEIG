import { http } from '../../lib/http.ts'

export interface ParametroPagamento {
  navigationId: string
  multaPercentual: number
  jurosDiarioPercentual: number
}

export function getParametroPagamento(signal?: AbortSignal) {
  return http<ParametroPagamento>('/api/ParametroPagamento', { signal })
}

export function updateParametroPagamento(payload: {
  multaPercentual: number
  jurosDiarioPercentual: number
}) {
  return http<ParametroPagamento>('/api/ParametroPagamento', { method: 'PUT', body: payload })
}
