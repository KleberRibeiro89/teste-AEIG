import { createCrudApi } from '../../api/createCrudApi.ts'
import { useCrud } from '../../hooks/useCrud.ts'
import type { BaseEntity } from '../../types/crud.ts'
import { asc } from '../../lib/query.ts'

export interface User extends BaseEntity {
  name: string
  email: string
}

export interface UserRequest {
  name: string
  email: string
  password: string
}

export const userApi = createCrudApi<User, UserRequest>('User')

export function useUsers() {
  return useCrud(userApi, {
    pageSize: 20,
    orderBy: [asc<User>('name')],
  })
}
