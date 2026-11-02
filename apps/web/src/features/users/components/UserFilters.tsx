import { Button, Collapse, Group, Paper, Select, Stack, TextInput } from '@mantine/core'
import { useState, type FormEvent } from 'react'
import { contains, eq } from '../../../lib/query.ts'
import type { PropertyFilter } from '../../../types/crud.ts'
import type { User } from '../userCrud.ts'

interface UserFiltersProps {
  opened: boolean
  onApply: (filters: PropertyFilter<User>[]) => void
  onClear: () => void
}

export function UserFilters({ opened, onApply, onClear }: UserFiltersProps) {
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [active, setActive] = useState<string | null>(null)

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const filters: PropertyFilter<User>[] = []
    const trimmedNome = nome.trim()
    const trimmedEmail = email.trim()

    if (trimmedNome) filters.push(contains<User>('nome', trimmedNome))
    if (trimmedEmail) filters.push(contains<User>('email', trimmedEmail))
    if (active === 'true' || active === 'false') filters.push(eq<User>('active', active === 'true'))
    onApply(filters)
  }

  function clear() {
    setNome('')
    setEmail('')
    setActive(null)
    onClear()
  }

  return (
    <Collapse expanded={opened}>
      <Paper component="form" onSubmit={submit} p="md" radius="md" bg="dark.6" withBorder>
        <Stack gap="sm">
          <TextInput label="Nome" value={nome} onChange={(event) => setNome(event.currentTarget.value)} />
          <TextInput label="E-mail" value={email} onChange={(event) => setEmail(event.currentTarget.value)} />
          <Select
            label="Situação"
            placeholder="Todas"
            clearable
            value={active}
            onChange={setActive}
            data={[
              { value: 'true', label: 'Ativo' },
              { value: 'false', label: 'Inativo' },
            ]}
          />
          <Group justify="flex-end">
            <Button type="button" variant="default" onClick={clear}>
              Limpar
            </Button>
            <Button type="submit">Filtrar</Button>
          </Group>
        </Stack>
      </Paper>
    </Collapse>
  )
}
