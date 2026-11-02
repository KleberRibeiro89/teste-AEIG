import {
  FilterCondition,
  type DynamicQuery,
  type FilterValue,
  type PropertyFilter,
  type PropertySort,
} from '../types/crud.ts'

export function eq<T>(name: Extract<keyof T, string>, value: FilterValue): PropertyFilter<T> {
  return { name, condition: FilterCondition.Equal, value }
}

export function contains<T>(name: Extract<keyof T, string>, value: string): PropertyFilter<T> {
  return { name, condition: FilterCondition.Contains, value }
}

export function asc<T>(name: Extract<keyof T, string>): PropertySort<T> {
  return { name, order: 'asc' }
}

export function desc<T>(name: Extract<keyof T, string>): PropertySort<T> {
  return { name, order: 'desc' }
}

export function toSearchParams<T>(query: DynamicQuery<T>): URLSearchParams {
  const params = new URLSearchParams()

  if (query.pageNumber !== undefined) params.set('pageNumber', String(query.pageNumber))
  if (query.pageSize !== undefined) params.set('pageSize', String(query.pageSize))
  if (query.operator !== undefined) params.set('operator', String(query.operator))
  if (query.filter?.length) params.set('filterString', JSON.stringify(query.filter))
  if (query.orderBy?.length) params.set('orderByString', JSON.stringify(query.orderBy))

  return params
}
