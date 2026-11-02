import { useCallback, useEffect, useState } from 'react'
import type { CrudApi } from '../api/createCrudApi.ts'
import {
  LogicalOperator,
  type BaseEntity,
  type DynamicQuery,
  type LogicalOperatorCode,
  type PagedResult,
  type PropertyFilter,
  type PropertySort,
} from '../types/crud.ts'

export interface UseCrudReturn<TEntity extends BaseEntity, TRequest> {
  data: PagedResult<TEntity>
  loading: boolean
  error: string | null
  query: DynamicQuery<TEntity>
  setPage(pageNumber: number): void
  setFilters(filter: PropertyFilter<TEntity>[], operator?: LogicalOperatorCode): void
  toggleSort(field: Extract<keyof TEntity, string>): void
  create(payload: TRequest): Promise<TEntity>
  update(navigationId: string, payload: TRequest): Promise<TEntity>
  remove(navigationId: string): Promise<void>
  reload(): void
}

const emptyPage = <T,>(pageSize: number): PagedResult<T> => ({
  pageSize,
  pageNumber: 0,
  totalRows: 0,
  result: [],
})

export function useCrud<TEntity extends BaseEntity, TRequest>(
  api: CrudApi<TEntity, TRequest>,
  initialQuery?: DynamicQuery<TEntity>,
): UseCrudReturn<TEntity, TRequest> {
  const [query, setQuery] = useState<DynamicQuery<TEntity>>({
    pageNumber: initialQuery?.pageNumber ?? 0,
    pageSize: initialQuery?.pageSize ?? 20,
    operator: initialQuery?.operator ?? LogicalOperator.And,
    filter: initialQuery?.filter ?? [],
    orderBy: initialQuery?.orderBy ?? [],
  })
  const [data, setData] = useState<PagedResult<TEntity>>(emptyPage(query.pageSize ?? 20))
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    api
      .getPaged(query, controller.signal)
      .then((page) => {
        setData(page)
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar os dados.')
        setLoading(false)
      })

    return () => controller.abort()
  }, [api, query, reloadToken])

  const reload = useCallback(() => setReloadToken((current) => current + 1), [])

  const setPage = useCallback((pageNumber: number) => {
    setQuery((current) => ({ ...current, pageNumber }))
  }, [])

  const setFilters = useCallback((filter: PropertyFilter<TEntity>[], operator?: LogicalOperatorCode) => {
    setQuery((current) => ({
      ...current,
      filter,
      operator: operator ?? current.operator,
      pageNumber: 0,
    }))
  }, [])

  const toggleSort = useCallback((field: Extract<keyof TEntity, string>) => {
    setQuery((current) => {
      const active = current.orderBy?.[0]
      let orderBy: PropertySort<TEntity>[] = [{ name: field, order: 'asc' }]
      if (active?.name === field && active.order === 'asc') orderBy = [{ name: field, order: 'desc' }]
      else if (active?.name === field && active.order === 'desc') orderBy = []
      return { ...current, orderBy, pageNumber: 0 }
    })
  }, [])

  const create = useCallback(
    async (payload: TRequest) => {
      const created = await api.create(payload)
      reload()
      return created
    },
    [api, reload],
  )

  const update = useCallback(
    async (navigationId: string, payload: TRequest) => {
      const updated = await api.update(navigationId, payload)
      reload()
      return updated
    },
    [api, reload],
  )

  const remove = useCallback(
    async (navigationId: string) => {
      await api.remove(navigationId)
      reload()
    },
    [api, reload],
  )

  return { data, loading, error, query, setPage, setFilters, toggleSort, create, update, remove, reload }
}
