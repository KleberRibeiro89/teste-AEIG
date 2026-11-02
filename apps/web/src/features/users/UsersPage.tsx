import { CrudTable, type CrudColumn } from '../../components/crud/CrudTable.tsx'
import { useUsers, type User } from './userCrud.ts'

const columns: CrudColumn<User>[] = [
  { key: 'name', header: 'Nome', sortable: true, filterable: true },
  { key: 'email', header: 'E-mail', sortable: true, filterable: true },
  { key: 'active', header: 'Ativo', sortable: true, filterable: true, render: (user) => (user.active ? 'Sim' : 'Não') },
]

export function UsersPage() {
  const users = useUsers()
  return <CrudTable title="Usuários" crud={users} columns={columns} />
}
