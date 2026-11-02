import { http } from '../lib/http.ts'
import { toSearchParams } from '../lib/query.ts'
import type { BaseEntity, DynamicQuery, PagedResult } from '../types/crud.ts'

export interface CrudApi<TEntity extends BaseEntity, TRequest> {
  resource: string
  getPaged(query?: DynamicQuery<TEntity>, signal?: AbortSignal): Promise<PagedResult<TEntity>>
  getById(navigationId: string, signal?: AbortSignal): Promise<TEntity>
  create(payload: TRequest): Promise<TEntity>
  update(navigationId: string, payload: TRequest): Promise<TEntity>
  remove(navigationId: string): Promise<void>
}

export function createCrudApi<TEntity extends BaseEntity, TRequest>(resource: string): CrudApi<TEntity, TRequest> {
  const root = `/api/${resource}`

  return {
    resource,
    getPaged: (query = {}, signal) =>
      http<PagedResult<TEntity>>(`${root}/paged`, { query: toSearchParams(query), signal }),
    getById: (navigationId, signal) => http<TEntity>(`${root}/${navigationId}`, { signal }),
    create: (payload) => http<TEntity>(root, { method: 'POST', body: payload }),
    update: (navigationId, payload) => http<TEntity>(`${root}/${navigationId}`, { method: 'PUT', body: payload }),
    remove: (navigationId) => http<void>(`${root}/${navigationId}`, { method: 'DELETE' }),
  }
}
