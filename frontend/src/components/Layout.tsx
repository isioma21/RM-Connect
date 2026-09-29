import { useState } from 'react'
import { AppBar, Box, Button, Container, Divider, Drawer, IconButton, Stack, Toolbar, Typography } from '@mui/material'
import MenuIcon from '@mui/icons-material/Menu'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { ConfirmDialog } from './ConfirmDialog'
import { IdleWarning } from './IdleWarning'

const customerLinks = [
  { to: '/customer', label: 'My manager' },
  { to: '/customer/book', label: 'Book appointment' },
  { to: '/security', label: 'Security' },
]

const managerLinks = [
  { to: '/manager', label: 'Customers' },
  { to: '/manager/schedule', label: 'Schedule' },
  { to: '/security', label: 'Security' },
]

const linkStyle = { color: 'text.secondary', justifyContent: 'flex-start', '&.active': { color: 'text.primary', fontWeight: 700 } }

/** Top bar and page area for signed-in pages. On phones the links move into a menu. */
export function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [confirmingLogout, setConfirmingLogout] = useState(false)
  const [menuOpen, setMenuOpen] = useState(false)
  const links = user?.role === 'Customer' ? customerLinks : managerLinks

  async function handleLogout() {
    setConfirmingLogout(false)
    await logout()
    navigate('/login')
  }

  function askLogout() {
    setMenuOpen(false)
    setConfirmingLogout(true)
  }

  return (
    <Box sx={{ minHeight: '100vh' }}>
      <AppBar position="static" color="inherit" elevation={0} sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Toolbar sx={{ gap: 3, px: { md: 6 } }}>
          <Typography variant="h3" component="div" color="primary" sx={{ flexGrow: { xs: 1, md: 0 } }}>
            RM Connect
          </Typography>
          <Stack component="nav" direction="row" spacing={1} sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
            {links.map((link) => (
              <Button key={link.to} component={NavLink} to={link.to} end sx={linkStyle}>{link.label}</Button>
            ))}
          </Stack>
          <Typography sx={{ fontWeight: 600, display: { xs: 'none', md: 'block' } }}>
            {user?.firstName} {user?.lastName}
          </Typography>
          <Button variant="outlined" color="inherit" onClick={askLogout} sx={{ display: { xs: 'none', md: 'inline-flex' } }}>
            Log out
          </Button>
          <IconButton aria-label="Open menu" onClick={() => setMenuOpen(true)} sx={{ display: { md: 'none' } }}>
            <MenuIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Drawer anchor="right" open={menuOpen} onClose={() => setMenuOpen(false)}>
        <Stack spacing={1} sx={{ width: 260, p: 2 }}>
          <Typography sx={{ fontWeight: 700, px: 1, py: 1 }}>{user?.firstName} {user?.lastName}</Typography>
          <Divider />
          <Stack component="nav" spacing={0.5}>
            {links.map((link) => (
              <Button key={link.to} component={NavLink} to={link.to} end sx={linkStyle} onClick={() => setMenuOpen(false)}>
                {link.label}
              </Button>
            ))}
          </Stack>
          <Divider />
          <Button variant="outlined" color="inherit" onClick={askLogout}>Log out</Button>
        </Stack>
      </Drawer>

      <ConfirmDialog
        open={confirmingLogout}
        title="Log out?"
        message="Are you sure you want to log out?"
        confirmLabel="Yes, log out"
        confirmColor="primary"
        onConfirm={handleLogout}
        onClose={() => setConfirmingLogout(false)}
      />

      <IdleWarning />

      <Container maxWidth="lg" sx={{ py: { xs: 3, md: 5 } }}>
        <Outlet />
      </Container>
    </Box>
  )
}
