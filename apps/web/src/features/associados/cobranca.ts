import { createCrudApi } from '../../api/createCrudApi.ts'
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

export const cobrancaApi = createCrudApi<Cobranca, never>('Cobranca')
