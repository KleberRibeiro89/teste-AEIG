import { createCrudApi } from '../../api/createCrudApi.ts'
import { useCrud } from '../../hooks/useCrud.ts'
import { asc } from '../../lib/query.ts'
import type { BaseEntity } from '../../types/crud.ts'

export interface Associado extends BaseEntity {
  name: string
  cpf: string
  monthlyFee: number
}

export interface AssociadoRequest {
  name: string
  cpf: string
  monthlyFee: number
}

export const associadoApi = createCrudApi<Associado, AssociadoRequest>('Associado')

export function useAssociados() {
  return useCrud(associadoApi, {
    pageSize: 20,
    orderBy: [asc<Associado>('name')],
  })
}
