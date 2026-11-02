import { createCrudApi } from '../../api/createCrudApi.ts'
import { useCrud } from '../../hooks/useCrud.ts'
import { asc } from '../../lib/query.ts'
import type { BaseEntity } from '../../types/crud.ts'

export interface Associado extends BaseEntity {
  nome: string
  cpf: string
  valorMensalidade: number
  diaVencimento: number
}

export interface AssociadoRequest {
  nome: string
  cpf: string
  valorMensalidade: number
  diaVencimento: number
}

export const associadoApi = createCrudApi<Associado, AssociadoRequest>('Associado')

export function useAssociados() {
  return useCrud(associadoApi, {
    pageSize: 20,
    orderBy: [asc<Associado>('nome')],
  })
}
