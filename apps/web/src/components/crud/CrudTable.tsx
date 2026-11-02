import { useState, type FormEvent, type ReactNode } from 'react'
import type { UseCrudReturn } from '../../hooks/useCrud.ts'
import { FilterCondition, filterConditionOptions, type BaseEntity, type FilterConditionCode, type FilterValue } from '../../types/crud.ts'
import './CrudTable.css'

export interface CrudColumn<T> {
  key: Extract<keyof T, string>
  header: string
  sortable?: boolean
  filterable?: boolean
  render?: (item: T) => ReactNode
}

interface CrudTableProps<TEntity extends BaseEntity, TRequest> {
  title: string
  crud: UseCrudReturn<TEntity, TRequest>
  columns: CrudColumn<TEntity>[]
}

function parseFilterValue(raw: string, condition: FilterConditionCode): FilterValue {
  const text = raw.trim()
  if (text === 'true') return true
  if (text === 'false') return false
  if (condition !== FilterCondition.Contains && text !== '' && !Number.isNaN(Number(text))) return Number(text)
  return text
}

export function CrudTable<TEntity extends BaseEntity, TRequest>({ title, crud, columns }: CrudTableProps<TEntity, TRequest>) {
  const filterable = columns.filter((column) => column.filterable)
  const [field, setField] = useState<Extract<keyof TEntity, string>>(filterable[0]?.key ?? columns[0].key)
  const [condition, setCondition] = useState<FilterConditionCode>(filterConditionOptions[0].value)
  const [value, setValue] = useState('')

  const pageSize = crud.data.pageSize || crud.query.pageSize || 20
  const pageNumber = crud.query.pageNumber ?? 0
  const totalPages = Math.max(1, Math.ceil(crud.data.totalRows / pageSize))
  const activeSort = crud.query.orderBy?.[0]

  const applyFilter = (event: FormEvent) => {
    event.preventDefault()
    const text = value.trim()
    if (!text) return
    crud.setFilters([...(crud.query.filter ?? []), { name: field, condition, value: parseFilterValue(text, condition) }])
    setValue('')
  }

  return (
    <section className="crud">
      <header className="crud-header">
        <h1>{title}</h1>
        <p>{loadingLabel(crud.loading, crud.data.totalRows)}</p>
      </header>

      {filterable.length > 0 && (
        <form className="crud-filters" onSubmit={applyFilter}>
          <select value={field} onChange={(event) => setField(event.target.value as Extract<keyof TEntity, string>)}>
            {filterable.map((column) => (
              <option key={column.key} value={column.key}>
                {column.header}
              </option>
            ))}
          </select>
          <select value={condition} onChange={(event) => setCondition(Number(event.target.value) as FilterConditionCode)}>
            {filterConditionOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          <input value={value} onChange={(event) => setValue(event.target.value)} placeholder="Valor" />
          <button type="submit" className="crud-button-primary">
            Filtrar
          </button>
          <button type="button" onClick={() => crud.setFilters([])}>
            Limpar
          </button>
        </form>
      )}

      {crud.error && <p className="crud-error">{crud.error}</p>}

      <div className="crud-table-wrap">
        <table>
          <thead>
            <tr>
              {columns.map((column) => {
                const sorted = activeSort?.name === column.key ? activeSort.order : undefined
                return (
                  <th key={column.key}>
                    {column.sortable ? (
                      <button type="button" onClick={() => crud.toggleSort(column.key)}>
                        {column.header}
                        {sorted === 'asc' ? ' ↑' : sorted === 'desc' ? ' ↓' : ''}
                      </button>
                    ) : (
                      column.header
                    )}
                  </th>
                )
              })}
            </tr>
          </thead>
          <tbody>
            {crud.data.result.map((item) => (
              <tr key={item.navigationId}>
                {columns.map((column) => (
                  <td key={column.key}>{column.render ? column.render(item) : String(item[column.key] ?? '')}</td>
                ))}
              </tr>
            ))}
            {!crud.loading && crud.data.result.length === 0 && (
              <tr>
                <td colSpan={columns.length}>Nenhum registro.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <footer className="crud-pager">
        <button type="button" disabled={pageNumber <= 0} onClick={() => crud.setPage(pageNumber - 1)}>
          Anterior
        </button>
        <span>
          Página {pageNumber + 1} de {totalPages}
        </span>
        <button type="button" disabled={pageNumber + 1 >= totalPages} onClick={() => crud.setPage(pageNumber + 1)}>
          Próxima
        </button>
      </footer>
    </section>
  )
}

function loadingLabel(loading: boolean, totalRows: number) {
  if (loading) return 'Carregando...'
  return `${totalRows} registro${totalRows === 1 ? '' : 's'}`
}
