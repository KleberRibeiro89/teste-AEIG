import { Button, Collapse, Group, NumberInput, Paper, Stack, TextInput } from '@mantine/core'
import { useState, type FormEvent } from 'react'
import { contains, eq } from '../../../lib/query.ts'
import type { PropertyFilter } from '../../../types/crud.ts'
import type { Associado } from '../associadoCrud.ts'

interface AssociadoFiltersProps {
  opened: boolean
  onApply: (filters: PropertyFilter<Associado>[]) => void
  onClear: () => void
}

export function AssociadoFilters({ opened, onApply, onClear }: AssociadoFiltersProps) {
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
