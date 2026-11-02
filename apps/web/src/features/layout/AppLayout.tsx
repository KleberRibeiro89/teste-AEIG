import { ActionIcon, AppShell } from '@mantine/core'
import { useDisclosure, useMediaQuery } from '@mantine/hooks'
import { type ReactNode } from 'react'
import { useLocation, useNavigate } from 'react-router'
import { ChevronIcon } from '../../components/icons.tsx'
import './AppLayout.css'
import { AppHeader } from './AppHeader.tsx'
import { AppNavbar } from './AppNavbar.tsx'

interface AppLayoutProps {
  children: ReactNode
  userName: string
  onLogout: () => void
}

export function AppLayout({ children, userName, onLogout }: AppLayoutProps) {
  const navigate = useNavigate()
  const location = useLocation()
  const [desktopCollapsed, { toggle: toggleDesktop }] = useDisclosure(false)
  const [mobileOpened, { toggle: toggleMobile, close: closeMobile }] = useDisclosure(false)
  const isMobile = useMediaQuery('(max-width: 48em)') ?? false
  const iconOnly = !isMobile && desktopCollapsed
  const menuOpen = isMobile ? mobileOpened : !desktopCollapsed
  const menuEdge = isMobile && !mobileOpened ? 0 : iconOnly ? 76 : 248

  return (
    <AppShell
      header={{ height: 64 }}
      navbar={{
        width: iconOnly ? 76 : 248,
        breakpoint: 'sm',
        collapsed: { mobile: !mobileOpened },
      }}
      padding="md"
    >
      <AppShell.Header className="app-header">
        <AppHeader onLogout={onLogout} />
      </AppShell.Header>

      <AppShell.Navbar className="app-navbar" p={iconOnly ? 'xs' : 'md'}>
        <AppNavbar
          pathname={location.pathname}
          iconOnly={iconOnly}
          userName={userName}
          onNavigate={(path) => {
            navigate(path)
            if (isMobile) closeMobile()
          }}
        />
      </AppShell.Navbar>

      <ActionIcon
        className="app-navbar-toggle"
        variant="filled"
        color="orange"
        size="sm"
        radius="xl"
        aria-label={menuOpen ? 'Recolher menu' : 'Expandir menu'}
        style={{
          left: menuEdge,
          transform: menuEdge === 0 ? 'translate(0, -50%)' : 'translate(-50%, -50%)',
        }}
        onClick={isMobile ? toggleMobile : toggleDesktop}
      >
        <ChevronIcon direction={menuOpen ? 'left' : 'right'} />
      </ActionIcon>

      <AppShell.Main className="app-main">{children}</AppShell.Main>
    </AppShell>
  )
}
