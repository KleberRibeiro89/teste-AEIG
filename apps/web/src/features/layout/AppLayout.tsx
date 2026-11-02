import { ActionIcon, AppShell, Avatar, Button, Group, NavLink, Text } from '@mantine/core'
import { useDisclosure, useMediaQuery } from '@mantine/hooks'
import { type ReactNode } from 'react'
import { useLocation, useNavigate } from 'react-router'
import logo from '../../assets/logo.png'
import './AppLayout.css'

interface AppLayoutProps {
  children: ReactNode
  userName: string
  onLogout: () => void
}

const menu: { id: string; label: string; path: string; icon: () => ReactNode }[] = [
  { id: 'users', label: 'Usuários', path: '/usuarios', icon: UsersIcon },
  { id: 'associados', label: 'Associados', path: '/associados', icon: AssociadoIcon },
]

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
        <Group h="100%" px="md" justify="space-between">
          <img src={logo} alt="AEIG" className="app-logo" />
          <Button variant="subtle" color="orange" onClick={onLogout}>
            Sair
          </Button>
        </Group>
      </AppShell.Header>

      <AppShell.Navbar className="app-navbar" p={iconOnly ? 'xs' : 'md'}>
        {menu.map((item) => {
          const Icon = item.icon
          return (
          <NavLink
            key={item.id}
            component="button"
            type="button"
            active={location.pathname === item.path || location.pathname.startsWith(`${item.path}/`)}
            label={iconOnly ? undefined : item.label}
            aria-label={item.label}
            leftSection={<Icon />}
            className={iconOnly ? 'app-navlink-icon' : undefined}
            onClick={() => {
              navigate(item.path)
              if (isMobile) closeMobile()
            }}
          />
          )
        })}
        <div className="app-navbar-user">
          <hr className="app-navbar-rule" />
          <Group gap="sm" wrap="nowrap" justify={iconOnly ? 'center' : 'flex-start'} title={userName}>
            <Avatar color="orange" radius="xl" size={iconOnly ? 'sm' : 'md'} alt={userName}>
              {initials(userName)}
            </Avatar>
            {!iconOnly && (
              <Text size="sm" fw={600} truncate="end">
                {userName}
              </Text>
            )}
          </Group>
        </div>
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

function initials(name: string) {
  const parts = name.trim().split(/\s+/).filter(Boolean)
  if (parts.length === 0) return '?'
  if (parts.length === 1) return parts[0].slice(0, 1).toUpperCase()
  return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase()
}

function ChevronIcon({ direction }: { direction: 'left' | 'right' }) {
  return (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <path
        d={direction === 'left' ? 'M15 6l-6 6 6 6' : 'M9 6l6 6-6 6'}
        stroke="currentColor"
        strokeWidth="2.4"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  )
}

function AssociadoIcon() {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <circle cx="12" cy="8" r="3" stroke="currentColor" strokeWidth="1.8" />
      <path d="M5.5 19c.7-3.2 3-5 6.5-5s5.8 1.8 6.5 5" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
    </svg>
  )
}

function UsersIcon() {
  return (
    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <circle cx="9" cy="8" r="3" stroke="currentColor" strokeWidth="1.8" />
      <path d="M3.5 19c.6-3 2.8-4.5 5.5-4.5S14.4 16 15 19" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
      <circle cx="17" cy="9" r="2.2" stroke="currentColor" strokeWidth="1.8" />
      <path d="M16.2 14.6c2.2.3 3.8 1.6 4.3 4.4" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
    </svg>
  )
}
