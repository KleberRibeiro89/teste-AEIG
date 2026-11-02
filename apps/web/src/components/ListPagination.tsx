import { Button, Group, Text } from '@mantine/core'

interface ListPaginationProps {
  pageNumber: number
  totalPages: number
  onPage: (pageNumber: number) => void
}

export function ListPagination({ pageNumber, totalPages, onPage }: ListPaginationProps) {
  if (totalPages <= 1) return null

  return (
    <Group gap="sm">
      <Button variant="default" disabled={pageNumber <= 0} onClick={() => onPage(pageNumber - 1)}>
        Anterior
      </Button>
      <Text size="sm">
        {pageNumber + 1} de {totalPages}
      </Text>
      <Button variant="default" disabled={pageNumber + 1 >= totalPages} onClick={() => onPage(pageNumber + 1)}>
        Próxima
      </Button>
    </Group>
  )
}
