import { Avatar } from '@mui/material'
import type { User } from '../api/types'

export function UserAvatar({ user, size = 52 }: { user: User; size?: number }) {
  return (
    <Avatar sx={{ width: size, height: size, bgcolor: 'primary.light', color: 'primary.main', fontWeight: 700 }}>
      {user.firstName[0]}{user.lastName[0]}
    </Avatar>
  )
}
