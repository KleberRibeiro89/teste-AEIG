import { Button, Group } from '@mantine/core'
import logo from '../../assets/logo.png'

interface AppHeaderProps {
  onLogout: () => void
}

export function AppHeader({ onLogout }: AppHeaderProps) {
  return (
    <Group h="100%" px="md" justify="space-between">
      <img src={logo} alt="AEIG" className="app-logo" />
      <Button variant="subtle" color="orange" onClick={onLogout}>
        Sair
      </Button>
    </Group>
  )
}
