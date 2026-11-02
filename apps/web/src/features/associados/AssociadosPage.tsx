import { Button, Group, Stack, Text, Title } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { useNavigate } from 'react-router'
import { ListPagination } from '../../components/ListPagination.tsx'
import { useAssociados } from './associadoCrud.ts'
import { AssociadoCard } from './components/AssociadoCard.tsx'
import { AssociadoFilters } from './components/AssociadoFilters.tsx'

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
          <AssociadoCard
            key={associado.navigationId}
            associado={associado}
            onOpen={() => navigate(`/associados/${associado.navigationId}`)}
          />
        ))}
      </Stack>

      <ListPagination pageNumber={pageNumber} totalPages={totalPages} onPage={associados.setPage} />
    </Stack>
  )
}
