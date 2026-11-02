import { Button, Collapse, Group, NumberInput, Paper, Stack, Text, TextInput, Title } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { contains, eq } from '../../lib/query.ts'
import type { PropertyFilter } from '../../types/crud.ts'
import { useAssociados, type Associado } from './associadoCrud.ts'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

function formatCpf(cpf: string) {
  const digits = cpf.replace(/\D/g, '')
  if (digits.length !== 11) return cpf
  return digits.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
}

export function AssociadosPage() {
  const associados = useAssociados()
  const navigate = useNavigate()
  const [filtersOpened, { toggle: toggleFilters }] = useDisclosure(false)
  const items = associados.data.result
  const pageSize = associados.data.pageSize || associados.query.pageSize || 20
  const pageNumber = associados.query.pageNumber ?? 0
  const totalPages = Math.max(1, Math.ceil(associados.data.totalRows / pageSize))

  return (
    <Stack gap="md" p="xl" maw={720}>
      <Group justify="space-between" align="flex-end">
        <div>
          <Title order={2}>Associados</Title>
          <Text c="dimmed" size="sm">
            {associados.loading
              ? 'Carregando...'
              : `${associados.data.totalRows} associado${associados.data.totalRows === 1 ? '' : 's'}`}
          </Text>
        </div>
        <Group gap="sm">
          <Button variant="default" onClick={toggleFilters} aria-expanded={filtersOpened}>
            Filtros
          </Button>
          <Button onClick={() => navigate('/associados/novo')}>Novo</Button>
        </Group>
      </Group>

      <AssociadoFilters
        opened={filtersOpened}
        onApply={associados.setFilters}
        onClear={() => associados.setFilters([])}
      />

      {associados.error && (
        <Text c="red" size="sm">
          {associados.error}
        </Text>
      )}

      {!associados.loading && !associados.error && items.length === 0 && (
        <Text c="dimmed">Nenhum associado cadastrado.</Text>
      )}

      <Stack gap="sm">
        {items.map((associado) => (
          <Paper key={associado.navigationId} p="md" radius="md" bg="dark.6" withBorder>
            <Text
              component="button"
              type="button"
              fw={600}
              c="orange"
              onClick={() => navigate(`/associados/${associado.navigationId}`)}
              style={{ background: 'none', border: 0, padding: 0, cursor: 'pointer', textAlign: 'left' }}
            >
              {associado.nome}
            </Text>
            <Text size="sm" c="dimmed">
              CPF {formatCpf(associado.cpf)}
            </Text>
            <Text size="sm" c="orange">
              Mensalidade {money.format(associado.valorMensalidade)}
            </Text>
            <Text size="sm" c="dimmed">
              Vencimento dia {associado.diaVencimento}
            </Text>
          </Paper>
        ))}
      </Stack>

      {totalPages > 1 && (
        <Group gap="sm">
          <Button variant="default" disabled={pageNumber <= 0} onClick={() => associados.setPage(pageNumber - 1)}>
            Anterior
          </Button>
          <Text size="sm">
            {pageNumber + 1} de {totalPages}
          </Text>
          <Button
            variant="default"
            disabled={pageNumber + 1 >= totalPages}
            onClick={() => associados.setPage(pageNumber + 1)}
          >
            Próxima
          </Button>
        </Group>
      )}

    </Stack>
  )
}

function AssociadoFilters({
  opened,
  onApply,
  onClear,
}: {
  opened: boolean
  onApply: (filters: PropertyFilter<Associado>[]) => void
  onClear: () => void
}) {
  const [name, setName] = useState('')
  const [cpf, setCpf] = useState('')
  const [monthlyFee, setMonthlyFee] = useState<number | string>('')
  const [dueDay, setDueDay] = useState<number | string>('')

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const filters: PropertyFilter<Associado>[] = []
    const trimmedName = name.trim()
    const digits = cpf.replace(/\D/g, '')
    const fee = typeof monthlyFee === 'number' ? monthlyFee : Number(String(monthlyFee).replace(',', '.'))
    const day = typeof dueDay === 'number' ? dueDay : Number(dueDay)

    if (trimmedName) filters.push(contains<Associado>('nome', trimmedName))
    if (digits) filters.push(contains<Associado>('cpf', digits))
    if (monthlyFee !== '' && Number.isFinite(fee)) filters.push(eq<Associado>('valorMensalidade', fee))
    if (dueDay !== '' && Number.isInteger(day)) filters.push(eq<Associado>('diaVencimento', day))
    onApply(filters)
  }

  function clear() {
    setName('')
    setCpf('')
    setMonthlyFee('')
    setDueDay('')
    onClear()
  }

  return (
      <Collapse expanded={opened}>
        <Paper component="form" onSubmit={submit} p="md" radius="md" bg="dark.6" withBorder>
          <Stack gap="sm">
            <TextInput label="Nome" value={name} onChange={(event) => setName(event.currentTarget.value)} />
            <TextInput label="CPF" value={cpf} onChange={(event) => setCpf(event.currentTarget.value)} />
            <NumberInput
              label="Mensalidade"
              value={monthlyFee}
              onChange={setMonthlyFee}
              min={0}
              decimalScale={2}
              prefix="R$ "
              decimalSeparator=","
              thousandSeparator="."
            />
            <NumberInput
              label="Dia de vencimento"
              value={dueDay}
              onChange={setDueDay}
              min={1}
              max={30}
              allowDecimal={false}
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
