export interface BaseEntity {
  navigationId: string
  createdAt: string
  createdBy?: string | null
  updatedAt?: string | null
  updatedBy?: string | null
  active: boolean
}

/** Números de System.Linq.Expressions.ExpressionType usados pelo backend. */
export const FilterCondition = {
  Equal: 13,
  GreaterThan: 15,
  GreaterThanOrEqual: 16,
  LessThan: 20,
  LessThanOrEqual: 21,
  NotEqual: 35,
  Contains: 70,
} as const

export type FilterConditionCode = (typeof FilterCondition)[keyof typeof FilterCondition]

export const LogicalOperator = {
  And: 2,
  Or: 36,
} as const

export type LogicalOperatorCode = (typeof LogicalOperator)[keyof typeof LogicalOperator]

export type FilterValue = string | number | boolean | null

export interface PropertyFilter<T> {
  name: Extract<keyof T, string>
  condition: FilterConditionCode
  value: FilterValue
}

export interface PropertySort<T> {
  name: Extract<keyof T, string>
  order: 'asc' | 'desc'
}

export interface DynamicQuery<T> {
  pageNumber?: number
  pageSize?: number
  operator?: LogicalOperatorCode
  filter?: PropertyFilter<T>[]
  orderBy?: PropertySort<T>[]
}

export interface PagedResult<T> {
  pageSize: number
  pageNumber: number
  totalRows: number
  result: T[]
}

export const filterConditionOptions: { value: FilterConditionCode; label: string }[] = [
  { value: FilterCondition.Contains, label: 'Contém' },
  { value: FilterCondition.Equal, label: 'Igual a' },
  { value: FilterCondition.NotEqual, label: 'Diferente de' },
  { value: FilterCondition.GreaterThan, label: 'Maior que' },
  { value: FilterCondition.GreaterThanOrEqual, label: 'Maior ou igual' },
  { value: FilterCondition.LessThan, label: 'Menor que' },
  { value: FilterCondition.LessThanOrEqual, label: 'Menor ou igual' },
]
