import { createCrudApi } from '../../api/createCrudApi.ts'
import { http } from '../../lib/http.ts'
import type { BaseEntity } from '../../types/crud.ts'

export interface Cobranca extends BaseEntity {
  idAssociado: number
  nossoNumero: string
  mesReferencia: string
  valor: number
  valorPago: number
  dataVencimento: string
  dataPagamento?: string | null
  idStatus: number
  nomeStatus?: string | null
}

export interface Pagamento {
  navigationId: string
  dataPagamento: string
  diasAtraso: number
  multa: number
  juros: number
  valorOriginal: number
  valorPago: number
  nomeStatus?: string | null
}

export const cobrancaApi = createCrudApi<Cobranca, never>('Cobranca')

export function pagarCobranca(navigationId: string, payload: { dataPagamento: string; juros: number }) {
  return http<Pagamento>(`/api/Cobranca/${navigationId}/pagar`, { method: 'POST', body: payload })
}
