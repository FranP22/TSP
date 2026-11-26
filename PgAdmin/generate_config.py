import json
import os
import stat
import pwd
import grp

POSTGRES_DB = os.environ["POSTGRES_DB"]
POSTGRES_USER = os.environ["POSTGRES_USER"]
POSTGRES_PASSWORD = os.environ["POSTGRES_PASSWORD"]

config = {
    "Servers": {
        "1": {
            "Name": "TSP PostgreSQL",
            "Group": "Servers",
            "Host": "db",
            "Port": 5432,
            "MaintenanceDB": POSTGRES_DB,
            "Username": POSTGRES_USER,
            "SSLMode": "prefer",
            "PassFile": "/var/lib/pgadmin/pgpass"
        }
    }
}

os.makedirs("/pgadmin4", exist_ok=True)
os.makedirs("/var/lib/pgadmin", exist_ok=True)

with open("/pgadmin4/servers.json", "w") as file:
    json.dump(config, file, indent=4)

def escape_pgpass(value):
    return value.replace("\\", "\\\\").replace(":", "\\:")

pgpass = (
    f"db:5432:*:"
    f"{escape_pgpass(POSTGRES_USER)}:"
    f"{escape_pgpass(POSTGRES_PASSWORD)}\n"
)

with open("/var/lib/pgadmin/pgpass", "w") as file:
    file.write(pgpass)

uid = pwd.getpwnam("pgadmin").pw_uid

try:
    gid = grp.getgrnam("pgadmin").gr_gid
except KeyError:
    gid = pwd.getpwnam("pgadmin").pw_gid

os.chown("/var/lib/pgadmin/pgpass", uid, gid)

os.chown("/var/lib/pgadmin/pgpass", uid, gid)
os.chmod("/var/lib/pgadmin/pgpass", stat.S_IRUSR | stat.S_IWUSR)

print("pgAdmin configuration generated successfully.")

os.execv("/entrypoint.sh", ["/entrypoint.sh"])