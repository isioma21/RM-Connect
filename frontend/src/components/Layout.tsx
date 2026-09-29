import { useState } from 'react'
import { AppBar, Box, Button, Container, Stack, Toolbar, Typography } from '@mui/material'
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

/** Top bar and page area for signed-in pages. */
export function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [confirmingLogout, setConfirmingLogout] = useState(false)
  const links = user?.role === 'Customer' ? customerLinks : managerLinks

  async function handleLogout() {
    setConfirmingLogout(false)
    await logout()
    navigate('/login')
  }

  return (
    <Box sx={{ minHeight: '100vh' }}>
      <AppBar position="static" color="inherit" elevation={0} sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Toolbar sx={{ gap: 3, px: { md: 6 } }}>
          <Typography variant="h3" component="div" color="primary">
            RM Connect
          </Typography>
          <Stack component="nav" direction="row" spacing={1} sx={{ flexGrow: 1 }}>
            {links.map((link) => (
              <Button
                key={link.to}
                component={NavLink}
                to={link.to}
                end
                sx={{ color: 'text.secondary', '&.active': { color: 'text.primary', fontWeight: 700 } }}
              >
                {link.label}
              </Button>
            ))}
          </Stack>
          <Typography sx={{ fontWeight: 600 }}>
            {user?.firstName} {user?.lastName}
          </Typography>
          <Button variant="outlined" color="inherit" onClick={() => setConfirmingLogout(true)}>
            Log out
          </Button>
        </Toolbar>
      </AppBar>

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

      <Container maxWidth="lg" sx={{ py: 5 }}>
        <Outlet />
      </Container>
    </Box>
  )
}
